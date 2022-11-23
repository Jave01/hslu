<!DOCTYPE html>
<html>
    <head>
        <title>Berechnetes Idealgewicht</title>
        <meta charset="utf8">
    </head>
    <body>
        <?php

            // TODO: Übernehmen Sie hier Funktion berechneIdealgewicht aus Übung 3 der Serie
            //       "Interaktive Web-Applikationen" und nehmen Sie Anpassungen vor, welche
            //       für PHP notwendig sind.
            function berechneIdealgewicht() {
                $groesse_field = $_POST["groesse"];
                $geschlecht = $_POST['geschlecht'];
                echo('<p>'. 'Hello world: ' . $_POST['geschlecht'] . '</p>');
                $idealgewicht = groesse - 100;

                // TODO: Rückgabe Ergebnis
                return geschlecht === "m" ? idealgewicht : 0.9 * idealgewicht;
            }

            function pruefeParameter() {
                if (!isset($_POST['geschlecht'])) {
                    echo "<p>Parameter 'geschlecht' wird benötigt</p>";
                    return false;
                }
                if (!isset($_POST['groesse'])) {
                    echo "<p>Parameter 'groesse' wird benötigt</p>";
                    return false;
                }
                return true;}

            if (pruefeParameter()) {
                // TODO: Parameter auslesen und Berechnung des Idealgewichts

                // TODO: Ausgabe des Idealgewichts
            }
            echo "<p>Parameter 'name' nicht gesetzt</p>";
            berechneIdealgewicht();
        ?>
    <h1>Test</h1>
    <a href="IdealgewichtForm.html">Zurück zum Formular</a>
    </body>
</html>