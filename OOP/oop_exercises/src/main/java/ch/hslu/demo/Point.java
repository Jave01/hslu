/*
 * Copyright 2022 Roland Gisler, HSLU Informatik, Switzerland
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package ch.hslu.demo;

import java.util.Objects;

/**
 * Modell eines Punktes im 2D-Raum.
 * Klasse ist Immutable implementiert.
 */
@SuppressWarnings("PMD.ShortVariable")
public final class Point {

    /**
     * Punkt keinem Quadrant zugeordnet.
     */
    private static final int NO_QUADRANT = 0;

    /**
     * Quadrant 1. Positiv x, Positiv y.
     */
    private static final int QUADRANT_1 = 1;

    /**
     * Quadrant 1. Negativ x, Positiv y.
     */
    private static final int QUADRANT_2 = 2;

    /**
     * Quadrant 1. Negativ x, Negativ y.
     */
    private static final int QUADRANT_3 = 3;

    /**
     * Quadrant 4. Positiv x, Negativ y.
     */
    private static final int QUADRANT_4 = 4;

    private final int x;
    private final int y;

    /**
     * Constructor for point with coordinates.
     * @param x x-Koordinate.
     * @param y y-Koordinate.
     */
    public Point(final int x, final int y) {
        this.x = x;
        this.y = y;
    }

    /**
     * Liefert den Quadranten in welchem der Punkt liegt.
     * Liegen die Koordinaten auf den Achsen oder
     * auf dem Nullpunkt, liefert die Funktion den Wert 0 zurueck.
     * @return Nummer des Quadranten.
     */
    public int getQuadrant() {
        int quadrant = NO_QUADRANT;
        if (this.x != 0 && this.y != 0) {
            if (this.x > 0) {
                if (this.y > 0) {
                    quadrant = QUADRANT_1;
                } else {
                    quadrant = QUADRANT_4;
                }
            } else {
                if (this.y > 0) {
                    quadrant = QUADRANT_2;
                } else {
                    quadrant = QUADRANT_3;
                }
            }
        }
        return quadrant;
    }

    public int getQuadrant1(){
        if (this.x == 0 && this.y == 0)
            return NO_QUADRANT;

        if (this.x * this.y > 0){
            if(this.x > 0){
                return QUADRANT_1;
            } else{
                return QUADRANT_3;
            }
        } else {
            if (this.x > 0){
                return QUADRANT_2;
            } else {
                return  QUADRANT_4;
            }
        }
    }

    /**
     * Returns x-coordinate.
     * @return x-coordinate of point.
     */
    public int getX() {
        return this.x;
    }

    /**
     * Liefert y-Koordinate zurueck.
     * @return y-Koordinate des Punktes.
     */
    public int getY() {
        return y;
    }

    /**
     * Zwei Punkte mit identischen Koordinaten sind gleich.
     * {@inheritDoc}.
     */
    @Override
    public boolean equals(final Object obj) {
        if (this == obj) {
            return true;
        }
        if (!(obj instanceof Point)) {
            return false;
        }
        final Point other = (Point) obj;
        return this.x == other.x && this.y == other.y;
    }

    /**
     * Liefert Hashcode auf Basis von x und y-Koordinate.
     * {@inheritDoc}.
     */
    @Override
    public int hashCode() {
        return Objects.hash(this.x, this.y);
    }

    /**
     * Liefert eine String-Repraesentation des Punktes.
     * {@inheritDoc}.
     */
    @Override
    public String toString() {
        return "Point[x=" + this.x + ",y=" + this.y + "]";
    }

}
