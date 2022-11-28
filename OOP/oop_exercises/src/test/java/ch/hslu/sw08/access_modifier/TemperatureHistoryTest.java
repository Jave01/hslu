package ch.hslu.sw08.access_modifier;

import ch.hslu.sw05.chemistry.TemperatureType;
import ch.hslu.sw08.collections.TemperatureHistory;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class TemperatureHistoryTest {
    @Test
    void testMin(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.add(new Temperature(5, TemperatureType.KELVIN));
        tHistory.add(new Temperature(-3, TemperatureType.KELVIN));
        tHistory.add(new Temperature(10, TemperatureType.KELVIN));

        assertEquals(-3, tHistory.getMinKelvin());
    }

    @Test
    void testAverage(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.add(new Temperature(5, TemperatureType.CELSIUS));
        tHistory.add(new Temperature(6, TemperatureType.CELSIUS));
        tHistory.add(new Temperature(7, TemperatureType.CELSIUS));

        assertEquals(6, tHistory.getAverageCelsius());
    }

    @Test
    void testMinWithoutVal(){
        TemperatureHistory tHistory = new TemperatureHistory();

        assertEquals(0, tHistory.getMinKelvin());
    }

    @Test
    void testAverageWithoutVal(){
        TemperatureHistory tHistory = new TemperatureHistory();

        assertEquals(0, tHistory.getAverageCelsius());
    }
}