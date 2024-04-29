package ch.hslu.sw10.vehicles;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

import static org.junit.jupiter.api.Assertions.*;

class MotorTest {
    // don't forget to reset this variable before every test.
    private PropertyChangeEvent pcEvt = null;

    @BeforeEach
    public void resetEventTriggeredVariable(){
        pcEvt = null;
    }

    @Test
    public void testSwitchOn(){
        Motor motor = new Motor();
        assertTrue(motor.isSwitchedOff());
        motor.switchOn();
        assertTrue(motor.isSwitchedOn());
    }

    @Test
    public void testSwitchOff(){
        Motor motor = new Motor();
        motor.switchOn();
        assertTrue(motor.isSwitchedOn());
        motor.switchOff();
        assertTrue(motor.isSwitchedOff());
    }

    @Test
    public void testStartRpm(){
        Motor motor = new Motor();
        motor.switchOn();
        assertEquals(1600, motor.getRpm());
    }

    @Test
    public void testPositiveAcceleration(){
        Motor motor = new Motor();
        motor.switchOn();
        motor.accelerate(400);
        assertEquals(2000, motor.getRpm());
    }

    @Test
    public void testNegativeAcceleration(){
        Motor motor = new Motor();
        motor.switchOn();
        motor.accelerate(-600);
        assertEquals(1000, motor.getRpm());
    }

    @Test
    public void testBelowZeroRpm(){
        Motor motor = new Motor();
        motor.switchOn();
        motor.accelerate(-2000);
        assertEquals(0, motor.getRpm());
    }

    @Test
    public void testIntegerOverflowRpm(){
        Motor motor = new Motor();
        motor.switchOn();
        motor.accelerate(Integer.MAX_VALUE);
        assertEquals(Integer.MAX_VALUE, motor.getRpm());
    }

    @Test
    public void testSwitchOnEvent(){
        Motor motor = new Motor();
        motor.addPropertyChangeListener(new PropertyChangeListener() {
            @Override
            public void propertyChange(PropertyChangeEvent evt) {
                pcEvt = evt;
            }
        });
        motor.switchOn();
        assertNotNull(pcEvt);
        assertEquals("status", pcEvt.getPropertyName());
        assertEquals("off", pcEvt.getOldValue());
        assertEquals("on", pcEvt.getNewValue());
    }

    @Test
    public void testSwitchOffEvent(){
        Motor motor = new Motor();
        motor.switchOn();
        motor.addPropertyChangeListener(new PropertyChangeListener() {
            @Override
            public void propertyChange(PropertyChangeEvent evt) {
                pcEvt = evt;
            }
        });
        motor.switchOff();
        assertNotNull(pcEvt);
        assertEquals("status", pcEvt.getPropertyName());
        assertEquals("on", pcEvt.getOldValue());
        assertEquals("off", pcEvt.getNewValue());
    }
}