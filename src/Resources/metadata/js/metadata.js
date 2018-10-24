/**
 * groupdocs.metadata Plugin
 * Copyright (c) 2018 Aspose Pty Ltd
 * Licensed under MIT.
 * @author Aspose Pty Ltd
 * @version 1.0.0
 */

/*
******************************************************************
******************************************************************
GLOBAL VARIABLES
******************************************************************
******************************************************************
*/
var metadata = {
    id: "",
    type: "",
    value: 0,
};
var metadataList = [];


$(document).ready(function () {

    /*
    ******************************************************************
    NAV BAR CONTROLS
    ******************************************************************
    */

    //////////////////////////////////////////////////
    // Fix for touchscreen devices required to detect if current touch event used for scroll or for click
    //////////////////////////////////////////////////
    var disable_click_flag = false;

    $(window).scroll(function () {
        disable_click_flag = true;

        clearTimeout($.data(this, 'scrollTimer'));

        $.data(this, 'scrollTimer', setTimeout(function () {
            disable_click_flag = false;
        }, 250));
    });

    //////////////////////////////////////////////////
    // Disable default download event
    //////////////////////////////////////////////////
    $('#gd-btn-download').off('touchstart click');

    //////////////////////////////////////////////////
    // Disable default open document event
    //////////////////////////////////////////////////
    $('.gd-modal-body').off('touchstart click', '.gd-filetree-name');

    //////////////////////////////////////////////////
    // Open value bar event
    //////////////////////////////////////////////////
    $(".gd-metadata-toggle").on('touchstart click', function () {
        $(".gd-metadata-wrapper").toggleClass("active");
    });


    //////////////////////////////////////////////////
    // Fix for tooltips of the dropdowns
    //////////////////////////////////////////////////
    $('#gd-download-val-container').on('touchstart click', function (e) {
        if ($(this).hasClass('open')) {
            $('#gd-btn-download-value').parent().find('.gd-tooltip').css('display', 'none');
        } else {
            $('#gd-btn-download-value').parent().find('.gd-tooltip').css('display', 'initial');
        }
    });

    //////////////////////////////////////////////////
    // Open document event
    //////////////////////////////////////////////////
    $('.gd-modal-body').on('touchstart click', '.gd-filetree-name', function (e) {
        e.preventDefault();
        if (!disable_click_flag) {
            e.stopPropagation();
            // make metadata list empty for the new document
            metadataList = [];
            $('#gd-metadata-toggle').prop('checked', false);
            var isDir = $(this).parent().find('.fa-folder').hasClass('fa-folder');
            if (isDir) {
                // if directory -> browse
                if (currentDirectory.length > 0) {
                    currentDirectory = currentDirectory + "/" + $(this).text();
                } else {
                    currentDirectory = $(this).text();
                }
                toggleModalDialog(false, '');
                loadFileTree(currentDirectory);
            } else {
                // if document -> open
                clearPageContents();
                documentGuid = $(this).attr('data-guid');
                toggleModalDialog(false, '');
                loadDocument(function (data) {
                    // Generate thumbnails
                    generatePagesTemplate(data, data.length, 'thumbnails-');
                });
                loadMetadata(documentGuid);
            }
        }
    });

    //////////////////////////////////////////////////
    // add metadata property event
    //////////////////////////////////////////////////   
    $('.gd-metadata-sidebar-expanded').on('touchstart click', '#gd-add-metadata', function (e) {
        $("#gd-metadata").append(getHtmlMetadata("", "", true));
    });

    //////////////////////////////////////////////////
    // save value event
    //////////////////////////////////////////////////
    $('.gd-metadata-sidebar-expanded').on('touchstart click', '#gd-save-metadata', saveData);

    //////////////////////////////////////////////////
    // delete value event
    //////////////////////////////////////////////////
    $('.gd-metadata-sidebar-expanded').on('touchstart click', '.gd-delete-metadata', function (e) {
        $(this).parent().parent().parent().remove();
    });

    //////////////////////////////////////////////////
    // Download event
    //////////////////////////////////////////////////
    $('#gd-btn-download-value > li').on('touchstart click', function (e) {
        download($(this));
    });
});

/*
******************************************************************
FUNCTIONS
******************************************************************
*/

/**
 * get current file netadata
 * @param {string} documentGuid - file guid
 */
function loadMetadata(documentGuid) {
    var data = {
        guid: documentGuid
    };
    var url = getApplicationPath('getMetadata');
    $.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (returnedData) {
            $('#gd-modal-spinner').hide();
            if (returnedData.message != undefined) {
                // open error popup
                printMessage(returnedData.message);
                return;
            }
            viewMetadata(returnedData.documentMetadata);
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            console.log(err.Message);
            // open error popup
            printMessage(err.message);
        }
    });
}

/**
 * Save current document metadata
 */
function saveData() {
    var url = getApplicationPath('saveData');
    // save data
    $.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (returnedData) {
            $('#gd-modal-spinner').hide();
            var result = "";
            if (returnedData.message != undefined) {
                // if password for document is incorrect return to previouse step and show error
                if (returnedData.message.toLowerCase().indexOf("password") != -1) {
                    $("#gd-password-required").html(returnedData.message);
                    $("#gd-password-required").show();
                } else {
                    // open error popup
                    printMessage(returnedData.message);
                }
                return;
            }
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            console.log(err.Message);
            // open error popup
            printMessage(err.message);
        }
    });
}

/**
 * delete metadata property
 * @param {Object} event - delete metadata button click event data
 */
function deleteMetadata(event) {

}

/**
 * Download document
 * @param {Object} button - Clicked download button
 */
function download(button) {
    if (typeof documentGuid != "undefined" && documentGuid != "") {
        // Open download dialog
        window.location.assign(getApplicationPath("downloadDocument/?path=") + documentGuid);
    } else {
        // open error popup
        printMessage("Please open document first");
    }
}

/**
* Render document metadata properties
* @param {string} metadata - json string with the metadata
*/
function viewMetadata(metadata) {
    $("#gd-metadata").html('');
    $('#gd-metadata-toggle').prop('checked', true);
    $(".gd-metadata-wrapper").toggleClass("active");
    $.each($.parseJSON(metadata), function (key, value) {
        if (value != "") {                      
                $("#gd-metadata").append(getHtmlMetadata(key, value, false));           
        } else {
            return true;
        }
    });
}

/**
    * Metadata property
    * @param {string} metadataId - id of the current metadata property
    * @param {string} metadataValue = value of the current metadata property
    * @param {boolean} newProperty - determines if its a new property of existed
    */
function getHtmlMetadata(metadataId, metadataValue, newProperty) {
    metadataId = (newProperty) ? '<input type="text" placeholder="Enter Metadata ID here" class="gd-metadata-input gd-metadata-id-input">' : metadataId;
    var property = (newProperty) ? '<input type="text" placeholder="Enter Metadata Value here" class="gd-metadata-input">' : "";
    if (metadataValue != "") {
        if (typeof metadataValue == "object") {
            $.each(metadataValue, function (key, value) {
                value = (value != "") ? value : "";
                property += '<input type="text" class="gd-metadata-input" value="' + value + '">';
            });

        } else {
            property = '<input type="text" class="gd-metadata-input" value="' + metadataValue + '">';
        }
    }
    return '<div class="gd-metadata">' +
                '<div class="gd-metadata-avatar">' +
                    '<span>' +
                        '<p class="gd-metadata-id">' +
                           metadataId +
                        '</p>' +
                        '<div class="gd-delete-metadata">' +
                            '<i class="fa fa-trash-o" aria-hidden="true"></i>' +
                        '</div>' +
                    '</span>' +
                '</div>' +
                '<div class="gd-metadata-value">' +
                     property +
                '</div>' +
            '</div>';
}

/*
******************************************************************
******************************************************************
GROUPDOCS.Metadata PLUGIN
******************************************************************
******************************************************************
*/
(function ($) {
    /*
    ******************************************************************
    STATIC VALUES
    ******************************************************************
    */
    var gd_navbar = '#gd-navbar';

    /*
    ******************************************************************
    METHODS
    ******************************************************************
    */
    var methods = {
        init: function (options) {
            // set defaults
            var defaults = {
                hidden: true,
                custom: true,
                multimedia: true,
                downloadCsv: true,
                downloadXml: true
            };

            options = $.extend(defaults, options);
            getHtmlDownloadPanel();
            // assembly metadata side bar html base
            $(".wrapper").append(getHtmlMetadataPanelBase);

            if (options.downloadCsv) {
                $("#gd-btn-download-value").append(getHtmlDownloadCsv());
            }

            if (options.downloadXml) {
                $("#gd-btn-download-value").append(getHtmlDownloadXml());
            }
        }
    };

    /*
    ******************************************************************
    INIT PLUGIN
    ******************************************************************
    */
    $.fn.metadata = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method' + method + ' does not exist on jQuery.metadata');
        }
    };

    function getHtmlMetadataPanelBase() {
        return '<div class="gd-metadata-wrapper">' +
					// open/close trigger button BEGIN
					'<input id="gd-metadata-toggle" class="gd-metadata-toggle" type="checkbox" />' +
					'<label for="gd-metadata-toggle" class="gd-lbl-metadata-toggle"></label>' +
					// open/close trigger button END
					'<div class="gd-metadata-sidebar-expanded gd-ui-tabs gd-ui-widget gd-ui-widget-content gd-ui-corner-all">' +
						'<div id="gd-tab-metadata" class="gd-metadata-content">' +
							'<div class="gd-viewport">' +
								'<h3 class="gd-com-heading gd-colon">Metadata:</h3>' +
								'<div class="gd-overview" id="gd-metadata">' +
									// metadata will be here
								'</div>' +
							'</div>' +
                            '<div id="gd-add-metadata" class="gd-add-metadata">' +
                                '<i class="fa fa-plus-circle"></i>' +
                            '</div>' +
							'<a  id="gd-save-metadata" class="gd-save-button" href="#">save</a>' +
						'</div>' +
					'</div>' +
				'</div>';
    }

    function getHtmlDownloadPanel() {
        var downloadBtn = $("#gd-btn-download");
        var defaultHtml = downloadBtn.html();
        var downloadDropDown = '<li class="gd-nav-toggle" id="gd-download-val-container">' +
									'<span id="gd-download-value">' +
										'<i class="fa fa-download"></i>' +
										'<span class="gd-tooltip">Download</span>' +
									'</span>' +
									'<span class="gd-nav-caret"></span>' +
									'<ul class="gd-nav-dropdown-menu gd-nav-dropdown" id="gd-btn-download-value">' +
										// download types will be here
									'</ul>' +
								'</li>';
        downloadBtn.html(downloadDropDown);
    }

    function getHtmlDownloadCsv() {
        return '<li id="gd-csv-download">Export to CSV</li>';
    }

    function getHtmlDownloadXml() {
        return '<li id="gd-xml-download">Export to XML</li>';
    }

    function getHtmlSavePanel() {
        return '<li id="gd-nav-save" class="gd-save"><i class="fa fa-floppy-o"></i><span class="gd-tooltip">Save</span></li>';
    }

})(jQuery);