<!DOCTYPE html>
<html>

<head>
    <title>Hallo</title>
    <meta charset="utf8">
</head>

<body>
    <?php
    // TODO: (2) Prüfen, ob Parameters 'name' gesetzt ist (ersetzen Sie true)
    if (!isset($_GET['name']))  {

        // TODO: (1) Ausgabe des Parameters 'name'
        echo ("<p>Hey there " . $_GET['name'] . "</p>");
    } else {
        echo "<p>Parameter 'name' nicht gesetzt</p>";
    }
    ?>
</body>

</html>