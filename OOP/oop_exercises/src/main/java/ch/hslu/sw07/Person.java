package ch.hslu.sw07;

import java.util.Objects;

public class Person {
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;

        if (!(o instanceof Person person)) return false;

        return  id == person.id &&
                lastName.equals(person.lastName) &&
                surname.equals(person.surname);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, lastName, surname);
    }

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
}
