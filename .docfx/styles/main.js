$( document ).ready(function() {

    var pathname = window.location.pathname;
    var origin   = window.location.origin;

    // Select current branch
    if(origin.includes("localhost:8080"))
    {
        $("#environment").append('<optgroup label="Environment"><option value="localhost">localhost</option></optgroup>');
        $("#environment").val("localhost")
    }
    else if(origin.includes("gdx-dev.dotbunny.com"))
    {
        $("#environment").val("dev")
    }
    else
    {
        $("#environment").val("main")
    }

    $("#environment").change(() => {
            switch ($("#environment").val()) {
                case "localhost":
                    window.url = "http://localhost:8080".concat(pathname);
                    break;
                case "main":
                    window.url = "http://gdx.dotbunny.com".concat(pathname);
                    break;
                case "dev":
                    window.url = "http://gdx-dev.dotbunny.com".concat(pathname);
                    break;
            }
        });
});