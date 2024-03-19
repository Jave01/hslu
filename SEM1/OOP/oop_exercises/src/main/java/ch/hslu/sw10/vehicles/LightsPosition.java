package ch.hslu.sw10.vehicles;

public enum LightsPosition {
    FRONT_LEFT("front left"), FRONT_RIGHT("front right");

    private String pos;

    private LightsPosition(String pos){
        this.pos = pos;
    }

    public String getPos(){
        return this.pos;
    }
}
