// Sometimes you have to use the right tool for the right job.
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

    // Setup change event for branch selection
    $("#environment").change(() => {
        switch ( $("#environment").val()) {

            case "localhost":
                window.location.href = "http://localhost:8080".concat(pathname);
                break;
            case "main":
                window.location.href = "http://gdx.dotbunny.com".concat(pathname);
                break;
            case "dev":
                window.location.href = "http://gdx-dev.dotbunny.com".concat(pathname);
                break;
        }
    });
});