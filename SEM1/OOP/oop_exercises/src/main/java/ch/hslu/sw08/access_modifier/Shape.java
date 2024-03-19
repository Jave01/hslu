package ch.hslu.sw08.access_modifier;

/**
 * Abstract class for implementing 2-dimensional shape-subclasses.
 */
public abstract class Shape {
    private int x;
    private int y;

    protected Shape(final int x, final int y) {
        this.x = x;
        this.y = y;
    }

    public final void move(final int x, final int y) {
        this.x += x;
        this.y += y;
    }

    public final int getX() {
        return x;
    }

    public final int getY() {
        return y;
    }

    /**
     * @return int perimeter
     */
    public abstract int getPerimeter();

    /**
     * @return int area.
     */
    public abstract int getArea();
}
