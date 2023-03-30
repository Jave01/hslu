<!DOCTYPE html>
<html>

<head>
    <title>Übung 4: Zufallszahl</title>
    <meta charset="utf8">
    <style>
        .red   { color: red; }
        .green { color: green; }
    </style>
</head>

<body>
    <form action="zufallszahl.php">
        <?php 
        $zahl = random_int(1, 100);

        // TODO: Paragraph mit Zahl ($zahl) ausgeben. Klasse gruen, falls < 50 sonst Klasse rot.
        $class = $zahl > 50 ? "red" : "green";  
        echo("<p class='" . $class . "'>" . $class . "</p>")
        ?>
        <button type="submit">berechne Zufallszahl</button>
    </form>
</body>

</html>