package ch.hslu.sw10.temperature;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;


import static org.junit.jupiter.api.Assertions.*;

class TemperatureHistoryTest {
    public static boolean eventTriggered = false;

    @BeforeEach
    public void resetVariable(){
        TemperatureHistoryTest.eventTriggered = false;
    }

    @Test
    public void testAddTemperature(){
        TemperatureHistory tHistory = new TemperatureHistory();
        Temperature exampleTemperature = Temperature.createFromKelvin(50);
        assertTrue(tHistory.add(exampleTemperature));
    }

    @Test
    public void testRemoveTemperature(){
        TemperatureHistory tHistory = new TemperatureHistory();
        Temperature exampleTemperature = Temperature.createFromKelvin(50);
        tHistory.add(exampleTemperature);
        assertTrue(tHistory.remove(exampleTemperature));
    }

    @Test
    public void testRemoveNonExistentTemperature(){
        TemperatureHistory tHistory = new TemperatureHistory();
        assertFalse(tHistory.remove(Temperature.createFromKelvin(0)));
    }

    @Test
    public void testNewMax(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.addTemperatureChangeListener(new TemperatureChangeListener(){
            @Override
            public void temperatureChange(TemperatureChangeEvent evt) {
                float measuredMaxKelvin = ((TemperatureHistory)evt.getSource()).getMaxKelvin();
                assertEquals(5, measuredMaxKelvin);
                TemperatureHistoryTest.eventTriggered = true;
            }
        });
        tHistory.add(Temperature.createFromKelvin(5));
        assertTrue(TemperatureHistoryTest.eventTriggered);
    }

    @Test
    public void testNewMin(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.add(Temperature.createFromKelvin(50));
        tHistory.addTemperatureChangeListener(new TemperatureChangeListener(){
            @Override
            public void temperatureChange(TemperatureChangeEvent evt) {
                float measuredMinKelvin = ((TemperatureHistory)evt.getSource()).getMinKelvin();
                assertEquals(10, measuredMinKelvin);
                TemperatureHistoryTest.eventTriggered = true;
            }
        });
        tHistory.add(Temperature.createFromKelvin(10));
        assertTrue(TemperatureHistoryTest.eventTriggered);
    }

    @Test
    public void testNewMinAfterRemove(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.add(Temperature.createFromKelvin(50));
        tHistory.add(Temperature.createFromKelvin(30));
        tHistory.add(Temperature.createFromKelvin(20));
        TemperatureChangeListener tcListener = new TemperatureChangeListener(){
            @Override
            public void temperatureChange(TemperatureChangeEvent evt) {
                float measuredMinKelvin = ((TemperatureHistory)evt.getSource()).getMinKelvin();
                assertEquals(30, measuredMinKelvin);
                TemperatureHistoryTest.eventTriggered = true;
            }
        };
        tHistory.addTemperatureChangeListener(tcListener);
        tHistory.remove(Temperature.createFromKelvin(20));

        assertTrue(TemperatureHistoryTest.eventTriggered);
    }

    @Test
    public void testNewMaxAfterRemove(){
        TemperatureHistory tHistory = new TemperatureHistory();
        tHistory.add(Temperature.createFromKelvin(50));
        tHistory.add(Temperature.createFromKelvin(30));
        tHistory.add(Temperature.createFromKelvin(20));
        TemperatureChangeListener tcListener = new TemperatureChangeListener(){
            @Override
            public void temperatureChange(TemperatureChangeEvent evt) {
                float measuredMaxKelvin = ((TemperatureHistory)evt.getSource()).getMaxKelvin();
                assertEquals(30, measuredMaxKelvin);
                TemperatureHistoryTest.eventTriggered = true;
            }
        };
        tHistory.addTemperatureChangeListener(tcListener);
        tHistory.remove(Temperature.createFromKelvin(50));

        assertTrue(TemperatureHistoryTest.eventTriggered);
    }
}