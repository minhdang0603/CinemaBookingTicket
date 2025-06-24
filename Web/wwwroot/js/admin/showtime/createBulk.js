$(document).ready(function () {
    const showtimes = [];
    
    // Time preset buttons click handler
    $(".time-preset").on("click", function() {
        const time = $(this).data("time");
        addShowtimeWithTime(time);
        
        // Toggle active class for visual feedback
        $(this).toggleClass("active");
    });
    
    // Add custom time button click handler
    $("#addCustomTimeBtn").on("click", function() {
        const startTime = $("#newStartTime").val();
        if (!startTime) {
            alert("Please select a time");
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
            alert("Please fill in all fields in the template");
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
            alert("This showtime is already added");
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
    $("#showtimesContainer").on("click", ".remove-btn", function() {
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
    
    // Submit button handler
    $("#submitShowtimesBtn").on("click", function() {
        if (showtimes.length === 0) {
            alert("Please add at least one showtime");
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
            success: function(response) {
                if (response.isSuccess) {
                    // Show success message
                    if (response.result && response.result.successfulShowTimes) {
                        const successCount = response.result.successfulShowTimes.length;
                        const failCount = response.result.failedShowTimes ? response.result.failedShowTimes.length : 0;
                        
                        if (failCount > 0) {
                            alert(`Created ${successCount} showtimes successfully, but ${failCount} failed.`);
                        } else {
                            alert(`Created all ${successCount} showtimes successfully!`);
                        }
                    }
                    
                    // Redirect to index page
                    window.location.href = "/Admin/Showtime/Index";
                } else {
                    // Show error message
                    let errorMsg = "Failed to create showtimes.";
                    if (response.errorMessages && response.errorMessages.length > 0) {
                        errorMsg = response.errorMessages.join("\n");
                    }
                    alert(errorMsg);
                    
                    // Reset button state
                    $btn.prop("disabled", false);
                    $btn.html('<i class="fas fa-save"></i> Create All Showtimes');
                }
            },
            error: function(xhr, status, error) {
                // Show error message
                let errorMsg = "Error creating showtimes: " + error;
                if (xhr.responseJSON && xhr.responseJSON.errorMessages) {
                    errorMsg = xhr.responseJSON.errorMessages.join("\n");
                }
                alert(errorMsg);
                
                // Reset button state
                $btn.prop("disabled", false);
                $btn.html('<i class="fas fa-save"></i> Create All Showtimes');
            }
        });
    });
});