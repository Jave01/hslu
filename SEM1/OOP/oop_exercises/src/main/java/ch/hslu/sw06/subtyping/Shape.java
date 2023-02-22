package ch.hslu.sw06.subtyping;

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

    public void move(final int x, final int y) {
        this.x += x;
        this.y += y;
    }

    public int getX() {
        return x;
    }

    public int getY() {
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
