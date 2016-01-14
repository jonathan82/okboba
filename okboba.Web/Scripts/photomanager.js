/*
 *  Author : Jonathan Lin
 *  Date   : 1/12/2016
 *  Notes  : Contains functions for editing thumbnail, deleting photos, uploading, and 
 *           re-arranging photos. Uses FileReaderJS plugin.
 * 
 *           Probably should use imagesLoaded plugin for more robust code.
 */
var PhotoManager = (function ($) {

    /////////////// Module vars //////////////////
    var configMap = {
        deletePhotoApi: '/photo/delete',
        deleteConfirmMsg: 'Are you sure you want to delete this photo?',
        errMsgTooBig: 'Upload file size must be less than 5MB!',
        errMsgNotImage: 'You must choose an image file!',
        errMsgTooSmall: 'Photo too small!',
        uploadPhotoTitle: 'Upload your photo',
        editThumbnailTitle: 'Choose your smiling face',
        uploadButtonText: 'Upload',
        editButtonText: 'Save',
        uploadAction: '/photo/upload',
        editAction: '/photo/editthumbnail',
        maxFileSize: 5000000, //5 MB
        minResolution: 300, //pixels
        uploadPhotoTemplateSel: '#uploadPhotoTemplate'
    };

    // Options for thumbnail selector and filereader
    var dragOpts = {
            containment: 'parent'
        },
        resizeOpts = {
            containment: 'parent',
            minWidth: 100,
            minHeight: 100,
            aspectRatio: true
        },
        fileReaderOpts = {
            on: {
                load: function (e,file) { loadPhoto(e,file,loadPhotoSuccess, loadPhotoError); }
            },
            error: function (e, file) {
                alert('filereader error');
            }
        }


    var theModal,
        uploadPhotoTemplate;

    //////////////// Private Methods ////////////////////
    function loadPhotoSuccess(img) {
        theModal.find('.photo-upload-label').hide();
        theModal.find('.photo-upload-container').show();
        theModal.find('button[type="submit"]').prop('disabled', false);
    }

    function loadPhotoError(msg) {
        alert(msg);
    }

    /*
     * Checks that a photo meets requirements like maximum size, minimum resolution, etc.
     * Returns an error string if there's an error, otherwise returns empty string. 
     * Takes two arguments event and file from the FileReader object.
     */
    function loadPhoto(e, file, successCallback, errorCallback) {
        var img;

        //Check Size
        if (file.size > configMap.maxFileSize) {
            errorCallback(configMap.errMsgTooBig);
            return;
        }
        //Check Type
        if (!file.type.match(/image.*/)) {
            errorCallback(configMap.errMsgNotImage);
            return;
        }
        //Check minimum resolution - we need to load image to get this
        img = theModal.find('img.photo-on-screen').attr("src", e.target.result).load(function () {

            if (this.width < configMap.minResolution || this.height < configMap.minResolution) {
                //Image too small
                errorCallback(configMap.errMsgTooSmall);
                return;
            }

            //Everything OK - call success callback with loaded image
            successCallback(img);
        });
    }

    function showUploadPhoto() {

        setupModal({
            title: configMap.uploadPhotoTitle,
            action: configMap.uploadAction,
            buttonText: configMap.uploadButtonText,
            isUpload: true
        });

        theModal.find('input[type="file"]').fileReaderJS(fileReaderOpts);

        theModal.modal();
    }

    function setThumbnailVars() {
        var thumb, imgWidth;

        thumb = theModal.find('.photo-thumbnail-selector');

        imgWidth = theModal.find('.photo-on-screen').width();

        //send the on-screen width. let the server code figure out scaling
        theModal.find('#topThumb').val(thumb.position().top);
        theModal.find('#leftThumb').val(thumb.position().left);
        theModal.find('#widthThumb').val(thumb.width());
        theModal.find('#photoScreenWidth').val(imgWidth);
    }

    function deletePhoto() {
        var response, photo, thumb;

        photo = $(this).data('photo');
        thumb = $(this).closest('.grid-item');

        response = confirm(configMap.deleteConfirmMsg);

        if (!response) return;

        //Delete the photo
        $.post(configMap.deletePhotoApi, { photo: photo }).fail(function () {
            alert('failed');
        });

        //optimistically remove photo
        thumb.remove();
    }

    function showEditThumbnail() {
        var src, photo;

        src = $(this).data('src');
        photo = $(this).data('photo');

        setupModal({
            title: configMap.editThumbnailTitle,
            action: configMap.editAction,
            buttonText: configMap.editButtonText,
            src: src,
            photo: photo,
            isUpload: false
        });
        
        theModal.find('.photo-upload-container').show();

        theModal.find('button[type="submit"]').prop('disabled', false);

        theModal.modal();
    }

    function setupModal(config) {
        theModal = $(uploadPhotoTemplate.render(config));

        $('body').append(theModal);

        theModal.find('.photo-thumbnail-selector').draggable(dragOpts).resizable(resizeOpts);

        theModal.on('hidden.bs.modal', function () {
            //destroy the modal when it is hidden
            theModal.remove();
            theModal = null;
        });

        //Submit handler
        theModal.find('form').submit(function () {
            setThumbnailVars();
            $(this).find('button[type="submit"]').ladda().ladda('start');
        });
    }

    //////////////// Public API //////////////////////
    function initModule(config) {
        configMap = $.extend(configMap, config);

        //setup handlers
        $('[data-toggle="deletephoto"]').click(deletePhoto);
        $('[data-toggle="editthumbnail"]').click(showEditThumbnail);
        $('[data-toggle="photoupload"]').click(showUploadPhoto);

        editThumbModal = $('#editThumbnailModal');

        uploadPhotoTemplate = $.templates(configMap.uploadPhotoTemplateSel);
    }

    return {
        initModule: initModule
    }

})(jQuery);