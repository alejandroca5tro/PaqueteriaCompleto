function CargarNotas()
{
    altair_notes.copy_list_sidebar(),
      altair_notes.init(),
    $sidebar_secondary_toggle.addClass("")
};
var $note_add_btn = $("#note_add"),
    $card_note = $(".md-card-single"),
    notes_list_class = ".notes_list",
    $notes_list = $("#notes_list");
altair_notes = {
    init: function () { altair_notes.add_new_note(), altair_notes.open_note() },
    add_new_note: function () {
        if ($note_add_btn)
        {
            var t = function () {
                $card_note.find("#note_title,#note_content").val(""), $notes_list.find("li").removeClass("md-list-item-active"), $card_note.find("#note_title").focus()
            }; $note_add_btn.on("click", function (e) {
                e.preventDefault(), altair_md.card_show_hide($card_note, void 0, t, void 0)
            })
        }
    }, open_note: function () {
        var t = function (t) {
            var e = t.attr("data-note-cuerpo");
            $card_note.find("#note_title").val(t.children(".md-list-heading").text()),
            $card_note.find("#note_content").val(e)
        }; $(notes_list_class).find("a").on("click", function (e) {
            e.preventDefault();
            var i = $(this); altair_md.card_show_hide($card_note, void 0, t, i), $(this).closest("li").siblings("li").removeClass("md-list-item-active").end().addClass("md-list-item-active")
        })
    }, copy_list_sidebar: function () { var t = $notes_list.clone(); t.attr("id", "notes_list_sidebar"), $sidebar_secondary.find(".sidebar_secondary_wrapper").html(t) }
};