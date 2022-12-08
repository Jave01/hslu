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
<<<<<<< HEAD
        tHistory.addTemperatureChangeListener(new TemperatureChangeListener() {
            public void temperatureChange(TemperatureChangeEvent evt) {
                logger.info("New " + evt.getTempEventType() + " temperature: " + evt.getEventTempValueKelvin() + "K");
            }
        });
=======
        tHistory.addTemperatureChangeListener(evt -> logger.info("New " + evt.tempEventType + " temperature"));
>>>>>>> 1b8a70bf0ef8bef1ebf82b14c9e83d1bb66c6b70

        // initial printout
        System.out.println("Enter temperature values in Kelvin or 'exit' for quitting:");

        // infinity loop until 'exit' is entered
        while(running){
            System.out.print("> ");
            input = scanner.nextLine();
            if("status".equals(input) || "exit".equals(input)){
                // if length equals 0 .getMin, .getMax and .getAverage wont work correctly.
                if(tHistory.getCount() > 0) {
                    logger.info("Count: " + tHistory.getCount() + ", " +
                            "average: " + Math.round(100 * tHistory.getAverageKelvin()) / 100 + " K, " +
                            "min: " + tHistory.getMinKelvin() + " K, " +
                            "max: " + tHistory.getMaxKelvin() + " K");
                }else {
                    logger.info("count: 0, average: 0, min: 0, max: 0");
                }
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
                    logger.error("Input is not a number nor 'exit'/'status'.");
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
