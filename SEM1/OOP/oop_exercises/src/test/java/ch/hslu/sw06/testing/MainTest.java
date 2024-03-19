package ch.hslu.sw06.testing;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class MainTest {
    private Main main;

    @Test
    void max() {
        int ref = 0;
        int values[] = { -2000000, -150, -5, -0, 5, 150, 200000 };

        this.main = new Main();
        assertEquals(ref, this.main.max(ref, values[0]));
        assertEquals(ref, this.main.max(ref, values[1]));
        assertEquals(ref, this.main.max(ref, values[2]));
        assertEquals(0, this.main.max(ref, values[3]));
        assertEquals(values[3], this.main.max(ref, values[3]));
        assertEquals(values[4], this.main.max(ref, values[4]));
        assertEquals(values[5], this.main.max(ref, values[5]));
        assertEquals(values[6], this.main.max(ref, values[6]));
    }
}