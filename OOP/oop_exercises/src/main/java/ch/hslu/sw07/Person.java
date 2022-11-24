package ch.hslu.sw07;

import java.util.Objects;

public class Person implements Comparable<Person>{
    private final int id;
    private String lastName;
    private String surname;

    public void setLastName(String lastName) {
        this.lastName = lastName;
    }

    public void setSurname(String surname) {
        this.surname = surname;
    }

    public String getLastName() {
        return lastName;
    }

    public String getSurname() {
        return surname;
    }

    public Person(int id, String surname, String lastName){
        this.id = id;
        this.surname = surname;
        this.lastName = lastName;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;

        if (!(o instanceof Person person)) return false;

        return  this.id == person.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }

    public int compareTo(Person other) {
        return Integer.compare(other.id, this.id);
    }
}
