package ch.hslu.sw06.chemistry;

/**
 * Specific lead-class, subclass from {@link ch.hslu.sw06.chemistry.AbsElement}
 */
public class Lead extends AbsElement {
    public Lead() {
        super(82, 600, 2024);
    }

    public String toString(){
        return "Lead: [Atomic number="+this.getAtomicNumber()+", melting at="+this.getMeltingPointKelvin()+"K, boiling at="+this.getBoilingPointKelvin()+"K]";
    }
}
