$(document).ready(function () {
    // Find the element by its class name
    var element = $('.dx-field-item-label-text:contains("Image")');

    // Check if the element is found before attempting to modify it
    if (element.length > 0) {
        // Add the style to hide the element
        element.css('display', 'none');
    }
});

let formInstance;

function FocusEditor(s, e) {
    window.setTimeout(function () {
        formInstance.getEditor("Email").focus();  
    }, 0);
}

function button_onClick(data) {
    var validationResult = DevExpress.validationEngine.validateGroup(data.validationGroup.group);
    if (validationResult.isValid) {
        data.component.option("disabled", true);
        data.component.option("text", " Checking");
        $("#button-indicator").dxLoadIndicator("option", "visible", true);
    }
}

function onInitialized(e) {
    formInstance = e.component;
}

