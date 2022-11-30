package ch.hslu.sw10.temperature;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.util.Scanner;

public class Main{
    public static final Logger logger = LogManager.getLogger(Main.class);
    public static void main(String[] args) {

        Scanner scanner = new Scanner(System.in);
        String input;
        boolean running = true;
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.addTemperatureChangeListener(new TemperatureChangeListener() {
            public void temperatureChange(TemperatureChangeEvent evt) {
                logger.info("New " + evt.tempEventType + " temperature");
            }
        });


        System.out.println("Enter temperature values in Kelvin or 'exit' for quitting:");

        while(running){
            System.out.print("> ");
            input = scanner.nextLine();
            if("status".equals(input) || "exit".equals(input)){
                logger.info("Count: " + tHistory.getCount() + ", " +
                        "average: " + Math.round(100 * tHistory.getAverageKelvin()) / 100 + " K, " +
                        "min: " + tHistory.getMinKelvin() + " K, " +
                        "max: " + tHistory.getMaxKelvin() + " K");
            }
            else {
                try {
                    // parse float and check for validity
                    float enteredTemp = Float.parseFloat(input);
                    if (enteredTemp >= 0) {
                        // add temperature to history
                        tHistory.add(Temperature.createFromKelvin(enteredTemp));
                    } else {
                        logger.error("Entered kelvin value below zero");
                    }
                } catch (NumberFormatException e) {
                    logger.error("Input is not a number nor 'exit'.");
                }
            }
            if ("exit".equals(input))
            {
                running = false;
                logger.info("Exiting loop");
            }
        }
    }
}
