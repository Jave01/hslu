package ch.hslu.sw06.testing;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class CalculatorTest {
    private final Calculator calc = new Calculator();

    @Test
    void add(){
        assertEquals(0, calc.add(-1, 1));
        assertEquals(-5, calc.add(-6, 1));
        assertEquals(125, calc.add(100, 25));
    }

    @Test
    void subtract() {
        assertEquals(-2, calc.subtract(-1, 1));
        assertEquals(-5, calc.subtract(1, 6));
        assertEquals(125, calc.subtract(150, 25));
        assertNotEquals(0, calc.subtract(5, 2));
    }

    @Test
    void multiply() {
        assertEquals(6, calc.multiply(2, 3));
        assertEquals(0, calc.multiply(0, 3));
        assertEquals(-50, calc.multiply(-2, 25));
        assertNotEquals(6, calc.multiply(0, 6));
    }

    @Test
    void divide() {
//        assertThrows(RuntimeException.class, calc.divide(5, 0));
        assertEquals(0, calc.divide(0, 5));
        assertEquals(2, calc.divide(8, 4));
    }
}