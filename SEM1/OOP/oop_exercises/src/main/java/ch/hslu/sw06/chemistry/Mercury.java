package ch.hslu.sw06.chemistry;

/**
 * Specific mercury-class, subclass from {@link ch.hslu.sw06.chemistry.AbsElement}
 */
public class Mercury extends AbsElement {

    public Mercury() {
        super(80, 234, 630);
    }

    public String toString(){
        return "Mercury: [Atomic number="+this.getAtomicNumber()+", melting at="+this.getMeltingPointKelvin()+"K, boiling at="+this.getBoilingPointKelvin()+"K] - Careful, highly toxic";
    }
}
