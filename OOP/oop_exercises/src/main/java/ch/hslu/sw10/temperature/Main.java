package ch.hslu.sw10.temperature;

import ch.hslu.sw05.chemistry.TemperatureType;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Scanner;

public class Main implements PropertyChangeListener {
    public static void main(String[] args) {
        Object o = new Object();

        Logger logger = LogManager.getLogger(Main.class);
        Scanner scanner = new Scanner(System.in);
        String input;
        boolean running = true;
        TemperatureHistory tHistory = new TemperatureHistory();
        System.out.println("Enter temperature values in Kelvin or 'exit' for quitting:");

        while(running){
            System.out.print("> ");
            input = scanner.nextLine();
            if (!"exit".equals(input)){
                try{
                    // parse float and check for validity
                    float enteredTemp = Float.parseFloat(input);
                    if(enteredTemp > 0){
                        // add temperature to history
                        tHistory.add(Temperature.createFromKelvin(enteredTemp));
                    } else {
                        logger.error("Entered kelvin value below zero");
                    }
                } catch (NumberFormatException e){
                    logger.error("Input is not a number nor 'exit'.");
                }
            }else{
                logger.info("Count: " + tHistory.getCount() + ", " +
                        "average: " + Math.round(100 * tHistory.getAverageKelvin()) / 100 + " K, " +
                        "min: " + tHistory.getMinKelvin() + " K, " +
                        "max: " + tHistory.getMaxKelvin() + " K");
                running = false;
                logger.info("Exiting loop");
            }
        }
    }

    @Override
    public void propertyChange(PropertyChangeEvent evt) {

    }
}
