package ch.hslu.sw11.temperature;

import ch.hslu.sw10.temperature.Temperature;
import ch.hslu.sw10.temperature.TemperatureHistory;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;


public class Main {
    public static void main(String[] args) {
        // System.out.println(System.getProperty("user.dir"));
        Logger logger = LogManager.getLogger();

        String fName = "temperature_values.csv";

        // final String fPath = "./src/main/java/ch/hslu/sw11/temperature" + fName;
        final String fPath = ".\\src\\main\\java\\ch\\hslu\\sw11\\temperature\\" + fName;

        // System.out.println(System.getProperty("user.dir") + fPath) ;

        File f = new File(fPath);

        if (f.exists()){
            try (BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(fPath)))) {
                String line;
                float tempVal;

                TemperatureHistory tHistory = new TemperatureHistory();

                while((line = br.readLine()) != null){
                    tempVal = Decoder.getTempValue(line);
                    tHistory.add(Temperature.createFromCelsius(tempVal));
                }
                System.out.println(tHistory.getMinCelsius());
                System.out.println(tHistory.getMaxCelsius());
                System.out.println(tHistory.getAverageCelsius());

            }
            catch (Exception e){
                logger.error("Error occurred while opening/reading/writing to file: " + e);
            }
        } else {
            logger.error("File does not exist");
        }


    }
}
