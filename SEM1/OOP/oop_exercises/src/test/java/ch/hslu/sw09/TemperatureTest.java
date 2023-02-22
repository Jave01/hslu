package ch.hslu.sw09;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class TemperatureTest {
    @Test
    public void testThrowsArgumentException(){
        assertThrows(IllegalArgumentException.class, () -> {
            Temperature.createFromKelvin(-5);
            }
        );
    }

    @Test
    public void testDoesNotThrowException(){
        assertDoesNotThrow(() -> {
            Temperature.createFromKelvin(5);
            }
        );
    }
}