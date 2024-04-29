package ch.hslu.sw09;

import org.apache.logging.log4j.LogManager;

import java.util.Scanner;
import org.apache.logging.log4j.Logger;

public class Main {
    private static Logger logger = LogManager.getLogger(Main.class);

    public static void main(String[] args) {
        String input = "";
        try (Scanner scanner = new Scanner(System.in)) {
            do {
                try {
                    System.out.println("Bitte Temperatur eingeben ('exit' zum Beenden): ");
                    input = scanner.next();
                } catch (NumberFormatException e) {
                    if (!input.equals("exit")) {
                        logger.error("Is not a Number: " + input);
                    }
                }
            } while (!input.equals("exit"));
        }
        System.out.println("Programm beendet.");
    }
}
