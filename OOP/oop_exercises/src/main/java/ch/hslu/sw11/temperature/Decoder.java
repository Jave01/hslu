package ch.hslu.sw11.temperature;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

final public class Decoder {
    public static Float getTempValue(String input){
        String[] entries = input.split(";");
        return Float.valueOf(entries[2]);
    }

    public static LocalDateTime getTimeStamp(String input){
        String[] entries = input.split(";");
        return LocalDateTime.parse(entries[1], DateTimeFormatter.ofPattern("\"yyyy/MM/dd HH:mm:ss\""));
    }
}
