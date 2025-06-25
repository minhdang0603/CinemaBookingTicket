$(document).ready(function () {
    const showtimes = [];

    // Time preset buttons click handler
    $(".time-preset").on("click", function () {
        const time = $(this).data("time");
        addShowtimeWithTime(time);

        // Toggle active class for visual feedback
        $(this).toggleClass("active");
    });

    // Add custom time button click handler
    $("#addCustomTimeBtn").on("click", function () {
        const startTime = $("#newStartTime").val();
        if (!startTime) {
            // Fallback to alert if toastr isn't defined
            (window.toastr || { error: alert }).error("Please select a time");
            return;
        }
        addShowtimeWithTime(startTime);
    });

    function addShowtimeWithTime(startTime) {
        const movieId = $("#movieId").val();
        const screenId = $("#screenId").val();
        const showDate = $("#showDate").val();
        const basePrice = $("#basePrice").val();

        // Validation
        if (!movieId || !screenId || !showDate || !basePrice) {
            // Fallback to alert if toastr isn't defined
            (window.toastr || { error: alert }).error("Please fill in all fields in the template");
            return;
        }

        // Create showtime object
        const showtime = {
            movieId: parseInt(movieId),
            screenId: parseInt(screenId),
            showDate: showDate,
            startTime: startTime,
            basePrice: parseFloat(basePrice)
        };

        // Check if this time is already added
        const isDuplicate = showtimes.some(s =>
            s.movieId === showtime.movieId &&
            s.screenId === showtime.screenId &&
            s.showDate === showtime.showDate &&
            s.startTime === showtime.startTime
        );

        if (isDuplicate) {
            // Fallback to alert if toastr isn't defined
            (window.toastr || { error: alert }).error("This showtime is already added");
            return;
        }

        // Add to array
        showtimes.push(showtime);

        // Sort showtimes by time
        showtimes.sort((a, b) => a.startTime.localeCompare(b.startTime));

        // Update UI
        updateShowtimesList();

        // Clear custom time input
        $("#newStartTime").val("");
    }

    // Remove showtime handler (using event delegation)
    $("#showtimesContainer").on("click", ".remove-btn", function () {
        const index = $(this).data("index");
        showtimes.splice(index, 1);
        updateShowtimesList();
    });

    // Update the showtimes list in the UI
    function updateShowtimesList() {
        const container = $("#showtimesContainer");

        // Clear container
        container.empty();

        if (showtimes.length === 0) {
            container.html(`
                <div class="text-muted text-center py-4" id="noShowtimesMsg">
                    <i class="fas fa-film fa-3x mb-3"></i>
                    <p>No showtimes added yet. Add showtimes using the template above.</p>
                </div>
            `);
            $("#submitShowtimesBtn").prop("disabled", true);
        } else {
            $("#submitShowtimesBtn").prop("disabled", false);

            // Add each showtime to UI
            showtimes.forEach((showtime, index) => {
                const movieText = $(`#movieId option[value="${showtime.movieId}"]`).text();
                const screenText = $(`#screenId option[value="${showtime.screenId}"]`).text();

                const showtimeRow = $(`
                    <div class="showtime-row">
                        <button type="button" class="remove-btn" data-index="${index}">
                            <i class="fas fa-times"></i>
                        </button>
                        <div class="row">
                            <div class="col-md-4">
                                <strong>Movie:</strong> ${movieText}
                            </div>
                            <div class="col-md-4">
                                <strong>Screen:</strong> ${screenText}
                            </div>
                            <div class="col-md-4">
                                <strong>Date:</strong> ${showtime.showDate}
                            </div>
                            <div class="col-md-4">
                                <strong>Start Time:</strong> ${showtime.startTime}
                            </div>
                            <div class="col-md-4">
                                <strong>Base Price:</strong> ${showtime.basePrice.toLocaleString()} VND
                            </div>
                        </div>
                    </div>
                `);

                container.append(showtimeRow);
            });
        }
    }


    // Function to show messages with fallback to alert
    function showMessage(message, isError = false) {
        // Check if toastr exists
        if (window.toastr) {
            if (isError) {
                toastr.error(message);
            } else {
                toastr.success(message);
            }
        } else {
            // Fallback to simple alert
            alert(message);
        }
    }
    

    
    // Function to show the result modal
    function showResultModal(successCount, errors = []) {
        // Update modal title and content based on results
        if (successCount > 0) {
            $("#showtimeResultModalTitle").text("Showtime Creation Successful");
            $("#showtimeSuccessMessage").text(`Successfully created ${successCount} showtimes.`).show();
        } else {
            if (errors && errors.length > 0) {
                $("#showtimeResultModalTitle").text("Showtime Creation Failed");
                $("#showtimeSuccessMessage").hide();
            } else {
                $("#showtimeResultModalTitle").text("Showtime Creation Result");
                $("#showtimeSuccessMessage").text("Showtimes were processed.").show();
            }
        }
        
        // Handle errors if any
        if (errors && errors.length > 0) {
            const $errorsContainer = $("#showtimeErrorsContainer");
            const $errorsList = $("#showtimeErrorsList");
            
            $errorsContainer.removeClass("d-none");
            $errorsList.empty();
            
            errors.forEach(error => {
                $errorsList.append(`<li>${error}</li>`);
            });
        } else {
            $("#showtimeErrorsContainer").addClass("d-none");
        }
        
        // Show the modal
        const resultModal = new bootstrap.Modal(document.getElementById('showtimeResultModal'));
        resultModal.show();
        
        // Set up the redirect button
        $("#goToShowtimeListBtn").off("click").on("click", function() {
            window.location.href = "/Admin/Showtime/Index";
        });
    }

    // Submit button handler
    $("#submitShowtimesBtn").on("click", function () {
        if (showtimes.length === 0) {
            showMessage("Please add at least one showtime", true);
            return;
        }

        // Format showtimes for API
        const showtimesToSend = showtimes.map(st => ({
            movieId: st.movieId,
            screenId: st.screenId,
            showDate: st.showDate,
            startTime: st.startTime,
            basePrice: st.basePrice,
            endTime: "00:00" // This will be calculated on server based on movie duration
        }));

        // Show loading state
        const $btn = $(this);
        $btn.prop("disabled", true);
        $btn.html('<i class="fas fa-spinner fa-spin"></i> Creating Showtimes...');

        // Get the anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();

        // Send API request
        $.ajax({
            url: "/Admin/Showtime/CreateBulk",
            type: "POST",
            data: JSON.stringify(showtimesToSend),
            contentType: "application/json; charset=utf-8",
            headers: {
                "RequestVerificationToken": token
            },
            success: function (response) {
                console.log("Response received:", response);

                if (response.isSuccess) {
                    // Get success count and prepare error messages
                    let successCount = 0;
                    let errorMessages = [];
                    
                    // Get success message from response
                    const successMessage = response.message || "Showtimes created successfully";
                    
                    // Get error messages if any (partial failures)
                    if (response.errorMessage) {
                        // Check if errorMessage is an array directly
                        if (Array.isArray(response.errorMessage)) {
                            errorMessages = response.errorMessage;
                        }
                        // Check if errorMessage has $values property (JSON.NET serialization format)
                        else if (response.errorMessage.$values) {
                            errorMessages = response.errorMessage.$values;
                        }
                    }
                    
                    // Extract success count from the message if available
                    const countMatch = successMessage.match(/Create (\d+) showtimes/);
                    if (countMatch && countMatch[1]) {
                        successCount = parseInt(countMatch[1], 10);
                    }
                    
                    // Show the result modal - no need for additional toastr messages
                    showResultModal(successCount, errorMessages);
                } else {
                    // Show error messages
                    let errorMessages = [];
                    
                    // Error messages could be in errorMessages or errorMessage
                    if (response.errorMessages) {
                        errorMessages = response.errorMessages;
                    } else if (response.errorMessage) {
                        // Handle both array and $values format
                        if (Array.isArray(response.errorMessage)) {
                            errorMessages = response.errorMessage;
                        } else if (response.errorMessage.$values) {
                            errorMessages = response.errorMessage.$values;
                        } else {
                            errorMessages = [response.errorMessage.toString()];
                        }
                    }
                    
                    // Show errors in modal - no need for additional toastr messages
                    showResultModal(0, errorMessages);
                    
                    // Reset button state
                    $btn.prop("disabled", false);
                    $btn.html('<i class="fas fa-save"></i> Create All Showtimes');
                }
            },
            error: function (xhr, status, error) {
                // Show error message
                console.error("AJAX Error:", xhr.responseText);

                let errorMsg = "Error creating showtimes: " + error;
                let errorMessages = [];
                
                if (xhr.responseJSON) {
                    const response = xhr.responseJSON;
                    
                    // Extract error messages directly based on response structure
                    if (response.errorMessages && response.errorMessages.length) {
                        errorMessages = response.errorMessages;
                    } else if (response.errorMessage) {
                        if (Array.isArray(response.errorMessage)) {
                            errorMessages = response.errorMessage;
                        } else if (response.errorMessage.$values) {
                            errorMessages = response.errorMessage.$values;
                        } else if (typeof response.errorMessage === 'string') {
                            errorMessages = [response.errorMessage];
                        } else {
                            errorMessages = [response.errorMessage.toString()];
                        }
                    } else if (response.message) {
                        errorMessages = [response.message];
                    } else {
                        errorMessages = [errorMsg];
                    }
                } else {
                    errorMessages = [errorMsg];
                }
                
                // Show errors in modal instead of toastr
                showResultModal(0, errorMessages);

                // Reset button state
                $btn.prop("disabled", false);
                $btn.html('<i class="fas fa-save"></i> Create All Showtimes');
            }
        });
    });
});