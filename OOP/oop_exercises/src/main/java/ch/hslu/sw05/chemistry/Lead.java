package ch.hslu.sw05.chemistry;

public class Lead extends AbsElement{
    private int pteNumber = 82;
    private int MeltingPointKelvin = 600;
    private int BoilingPointKelvin = 2022;

    public int getPteNumber() {
        return pteNumber;
    }

    public int getMeltingPointKelvin() {
        return MeltingPointKelvin;
    }

    @Override
    public AggregateState getAggregateState(int tempKelvin) {
        if (tempKelvin >= this.BoilingPointKelvin){
            return AggregateState.GASEOUS;
        } else if (tempKelvin >= MeltingPointKelvin){
            return AggregateState.FLUID;
        } else if (tempKelvin > 0){
            return AggregateState.SOLID;
        } else {
            return null;
        }
    }

    public int getBoilingPointKelvin() {
        return this.BoilingPointKelvin;
    }

}
