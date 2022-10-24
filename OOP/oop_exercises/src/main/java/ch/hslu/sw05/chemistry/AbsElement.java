package ch.hslu.sw05.chemistry;

public abstract class AbsElement {
    protected int pteNumber;
    protected int BoilingPointKelvin;
    protected int MeltingPointKelvin;

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
}
