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


$(document).ready(function(){

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
    // Open value bar event
    //////////////////////////////////////////////////
    $(".gd-metadata-metadata-toggle").on('touchstart click', function(){
		$(".gd-metadata-metadata-wrapper").toggleClass("active");
	});
	
	
    //////////////////////////////////////////////////
    // Fix for tooltips of the dropdowns
    //////////////////////////////////////////////////
    $('#gd-download-val-container').on('touchstart click', function (e) {
        if($(this).hasClass('open')){
            $('#gd-btn-download-value').parent().find('.gd-tooltip').css('display', 'none');
        }else{
            $('#gd-btn-download-value').parent().find('.gd-tooltip').css('display', 'initial');
        }
    });

	//////////////////////////////////////////////////
    // Open document event
    //////////////////////////////////////////////////
    $('.gd-modal-body').on('touchstart click', '.gd-filetree-name', function (e) {		
		if (disable_click_flag) {
		    e.preventDefault();
		    e.stopPropagation();		
		    // make metadata list empty for the new document
		    metadataList = [];
		    $('#gd-metadata-metadata-toggle').prop('checked', false);
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
		    }
		}
	});
	
	//////////////////////////////////////////////////
    // add metadata property event
    //////////////////////////////////////////////////
	
	//////////////////////////////////////////////////
    // enter value text event
    //////////////////////////////////////////////////
    $('.gd-metadata-sidebar-expanded').on('touchstart click', 'div.gd-value-text', function (e) {
		
	});
	
	//////////////////////////////////////////////////
    // save value event
    //////////////////////////////////////////////////
    $('.gd-metadata-sidebar-expanded').on('touchstart click', '#gd-save-metadata', saveData);	
	
	//////////////////////////////////////////////////
    // delete value event
    //////////////////////////////////////////////////
    $('.gd-metadata-sidebar-expanded').on('touchstart click', '.gd-delete-value', function (e) {
		
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
        success: function(returnedData) {
            $('#gd-modal-spinner').hide();
            var result = "";
            if(returnedData.message != undefined){
                // if password for document is incorrect return to previouse step and show error
                if(returnedData.message.toLowerCase().indexOf("password") != -1){                  
                    $("#gd-password-required").html(returnedData.message);
                    $("#gd-password-required").show();
                } else {
                    // open error popup
                    printMessage(returnedData.message);
                }
                return;
            }           
        },
        error: function(xhr, status, error) {
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
function deleteMetadata(event){
	
}

/**
* Add new value into the metadata bar
*/
function addProperty(){
    
}
 
/**
 * Download document
 * @param {Object} button - Clicked download button
 */
function download (button){
    if(typeof documentGuid != "undefined" && documentGuid != ""){
         // Open download dialog
         window.location.assign(getApplicationPath("downloadDocument/?path=") + documentGuid);
    } else {
         // open error popup
         printMessage("Please open document first");
    }
}

/*
******************************************************************
******************************************************************
GROUPDOCS.Metadata PLUGIN
******************************************************************
******************************************************************
*/
(function( $ ) {
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
        init : function( options ) {
            // set defaults
            var defaults = {
                isHidden: true,
                isCustom: true,
                isMultimedia: true
            };

            options = $.extend(defaults, options);		
			
			// assembly metadata side bar html base
			$(".wrapper").append(getHtmlMetadataPanelBase);		
        }
    };

    /*
    ******************************************************************
    INIT PLUGIN
    ******************************************************************
    */
    $.fn.metadata = function( method ) {
        if ( methods[method] ) {
            return methods[method].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method' +  method + ' does not exist on jQuery.metadata' );
        }
    };

    function getHtmlMetadataPanelBase() {
		return '<div class="gd-metadata-wrapper">'+
					// open/close trigger button BEGIN
					'<input id="gd-metadata-toggle" class="gd-metadata-toggle" type="checkbox" />'+
					'<label for="gd-metadata-toggle" class="gd-lbl-metadata-toggle"></label>'+
					// open/close trigger button END
					'<div class="gd-metadata-sidebar-expanded gd-ui-tabs gd-ui-widget gd-ui-widget-content gd-ui-corner-all">'+						
						'<div id="gd-tab-metadata" class="gd-metadata-content">'+							
							'<div class="gd-viewport">'+
								'<h3 class="gd-com-heading gd-colon">Metadata:</h3>'+
								'<div class="gd-overview" id="gd-metadata">'+									
									// metadata will be here
								'</div>'+
							'</div>'+							
							'<a  id="gd-save-metadata" class="gd-save-button gd-save-button-disabled" href="#">save</a>'+
						'</div>'+
					'</div>'+
				'</div>';
	}
		
	function getHtmlSavePanel(){
        return '<li id="gd-nav-save" class="gd-save"><i class="fa fa-floppy-o"></i><span class="gd-tooltip">Save</span></li>';
	}

})(jQuery);