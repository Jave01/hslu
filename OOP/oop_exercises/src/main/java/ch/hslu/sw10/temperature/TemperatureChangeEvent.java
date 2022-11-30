package ch.hslu.sw10.temperature;

import java.util.EventObject;

public class TemperatureChangeEvent extends EventObject {

    public final TemperatureEventType tempEventType;

    public TemperatureChangeEvent(Object source, TemperatureEventType tempEventType) {
        super(source);
        this.tempEventType = tempEventType;
    }
}
