package ch.hslu.sw07;

import nl.jqno.equalsverifier.EqualsVerifier;
import nl.jqno.equalsverifier.Warning;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;

class PersonTest {

    @Disabled
    @Test
    public void equalsContract() {
        // this.person = new Person(5, "David", "Jäggli");
        // Person random = new Person(5, "David", "Jäggli");
        // Person random1 = new Person(8, "", "Jäggli");
        //
        // assertEquals(this.person, this.person);
        // assertEquals(this.person, random);
        // assertNotEquals(this.person, random1);

        EqualsVerifier.simple().forClass(Person.class).suppress(Warning.NULL_FIELDS).verify();
    }
}