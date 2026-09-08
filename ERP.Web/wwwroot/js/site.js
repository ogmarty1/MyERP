// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    // Intercepts submission of any form.js-delete-form, shows a SweetAlert2
    // warning confirm (text supplied via data-confirm-* attributes), and only
    // re-submits the form if the user confirms.
    $(document).on('submit', 'form.js-delete-form', function (e) {
        var $form = $(this);

        if ($form.data('confirmed')) {
            return;
        }

        e.preventDefault();

        Swal.fire({
            title: $form.data('confirm-title'),
            text: $form.data('confirm-text'),
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: $form.data('confirm-button'),
            cancelButtonText: $form.data('cancel-button'),
            confirmButtonColor: '#dc3545',
            reverseButtons: true
        }).then(function (result) {
            if (result.isConfirmed) {
                $form.data('confirmed', true);
                $form.trigger('submit');
            }
        });
    });

    // Disables the submit button and shows a spinner once a form.js-submit-loading
    // is actually about to post, i.e. only after unobtrusive validation (if any)
    // reports the form valid, so a failed validation never leaves the button stuck.
    $(document).on('submit', 'form.js-submit-loading', function () {
        var $form = $(this);

        if ($form.data('validator') && !$form.valid()) {
            return;
        }

        var $btn = $form.find('button[type="submit"]').first();
        if ($btn.length === 0 || $btn.prop('disabled')) {
            return;
        }

        $btn.prop('disabled', true);
        $btn.prepend('<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>');
    });

    // Generic view/edit toggle for Details pages. Markup contract:
    // - a wrapper with class .js-details-form-wrapper
    // - inside it, a <form class="js-details-form"> whose editable inputs/selects/textareas
    //   carry class .erp-editable and start out disabled/readonly
    // - a .js-view-actions block (Edit button) and a .js-edit-actions block (Save/Cancel), one hidden at a time
    $(document).on('click', '.js-edit-toggle', function () {
        var $wrapper = $(this).closest('.js-details-form-wrapper');
        $wrapper.find('.js-details-form').addClass('editing')
            .find('.erp-editable').prop('disabled', false).prop('readonly', false);
        $wrapper.find('.js-ai-suggest-btn').prop('disabled', false);
        $wrapper.find('.js-view-actions').addClass('d-none');
        $wrapper.find('.js-edit-actions').removeClass('d-none');
    });

    $(document).on('click', '.js-edit-cancel', function () {
        var $wrapper = $(this).closest('.js-details-form-wrapper');
        var form = $wrapper.find('.js-details-form').get(0);
        if (form) {
            form.reset();
        }
        $wrapper.find('.js-details-form').removeClass('editing')
            .find('.erp-editable').prop('disabled', true).prop('readonly', true);
        $wrapper.find('.js-ai-suggest-btn').prop('disabled', true);
        $wrapper.find('.js-edit-actions').addClass('d-none');
        $wrapper.find('.js-view-actions').removeClass('d-none');
    });

    // AI Suggest buttons (Products/Customers/Orders Details pages): posts the button's
    // data-instruction/data-context to the assistant and fills the target textarea with
    // the generated text. The user still has to Save the form explicitly.
    $(document).on('click', '.js-ai-suggest-btn', function () {
        var $btn = $(this);
        if ($btn.prop('disabled')) {
            return;
        }

        var $target = $($btn.data('target'));
        var originalHtml = $btn.html();
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');

        fetch(window.assistantDraftUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': window.assistantAntiForgeryToken
            },
            body: JSON.stringify({
                instruction: $btn.data('instruction'),
                context: $btn.data('context') || ''
            })
        })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.success) {
                    $target.val(result.text);
                } else {
                    toastr.error(result.error || window.assistantErrorMessage);
                }
            })
            .catch(function () {
                toastr.error(window.assistantErrorMessage);
            })
            .finally(function () {
                $btn.prop('disabled', false).html(originalHtml);
            });
    });

    // Floating AI assistant chat widget (see Views/Shared/_AssistantWidget.cshtml).
    // History is kept in memory only for the current page - it resets on navigation
    // since this is a classic server-rendered MVC app with no client-side router.
    var assistantHistory = [];

    function appendAssistantMessage(role, text) {
        var $bubble = $('<div>').addClass('assistant-message').addClass(role === 'user' ? 'assistant-message-user' : 'assistant-message-bot').text(text);
        $('#assistantMessages').append($bubble);
        $('#assistantMessages').scrollTop($('#assistantMessages')[0].scrollHeight);
        return $bubble;
    }

    function sendAssistantMessage(text) {
        if (!text) {
            return;
        }

        appendAssistantMessage('user', text);
        var $thinking = appendAssistantMessage('bot', window.assistantThinkingMessage);

        fetch(window.assistantChatUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': window.assistantAntiForgeryToken
            },
            body: JSON.stringify({
                message: text,
                history: assistantHistory
            })
        })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (result.success) {
                    $thinking.text(result.reply);
                    assistantHistory.push({ role: 'user', content: text });
                    assistantHistory.push({ role: 'assistant', content: result.reply });
                    if (assistantHistory.length > 20) {
                        assistantHistory = assistantHistory.slice(-20);
                    }
                } else {
                    $thinking.text(result.error || window.assistantErrorMessage);
                }
            })
            .catch(function () {
                $thinking.text(window.assistantErrorMessage);
            });
    }

    $(document).on('submit', '#assistantForm', function (e) {
        e.preventDefault();
        var $input = $('#assistantInput');
        var text = $input.val().trim();
        $input.val('');
        sendAssistantMessage(text);
    });

    $(document).on('click', '.assistant-quick-action', function () {
        sendAssistantMessage($(this).data('prompt'));
    });
});
