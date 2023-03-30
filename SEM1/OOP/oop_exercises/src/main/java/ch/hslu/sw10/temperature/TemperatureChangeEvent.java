package ch.hslu.sw10.temperature;

import java.util.EventObject;

public class TemperatureChangeEvent extends EventObject {

    private final TemperatureEventType tempEventType;

    private final float eventTempValueKelvin;

    public TemperatureChangeEvent(Object source, TemperatureEventType tempEventType, float eventTempValueKelvin) {
        super(source);
        this.tempEventType = tempEventType;
        this.eventTempValueKelvin = eventTempValueKelvin;
    }

    public TemperatureEventType getTempEventType() {
        return this.tempEventType;
    }

    public float getEventTempValueKelvin() {
        return this.eventTempValueKelvin;
    }
}
