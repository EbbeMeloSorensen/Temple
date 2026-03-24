import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import { Link, useHistory, useParams } from "react-router-dom";
import { Button, Header, Segment } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponents";
import { useStore } from "../../../app/stores/store";
import { v4 as uuid } from "uuid";
import { Formik, Form } from "formik";
import * as Yup from "yup";
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { PersonFormValues } from "../../../app/models/person";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { deadOptions } from "../../../app/common/options/deadOptions";

export default observer(function PersonForm() {
  const history = useHistory();
  const { personStore } = useStore();
  const { createPerson, updatePerson, loadPerson, loadingInitial } =
    personStore; // "Destructure the props we need from the person store"
  const { id } = useParams<{ id: string }>();

  const [person, setPerson] = useState<PersonFormValues>(
    new PersonFormValues()
  );

  const validationSchema = Yup.object({
    firstName: Yup.string().required(
      "The first name of the person is required"
    ),
    //surname: Yup.string().required('The surname of the person is required'),
    //description: Yup.string().required('The person description is required'),
    //birthday: Yup.string().required('Birthday is required').nullable()
  });

  useEffect(() => {
    if (id)
      loadPerson(id).then((person) => {
        //console.log("About to populate PersonForm with data");
        let pfv = new PersonFormValues(person);
        // Dead skal sættes som en string eller som null for at den vises i formen
        pfv.dead =
          pfv.dead === null || pfv.dead.toString() === ""
            ? null
            : pfv.dead.toString();
        setPerson(pfv);
      });
  }, [id, loadPerson]);

  function handleFormSubmit(person: PersonFormValues) {
    if (!person.id) {
      console.log("creating new person..");

      let newPerson = {
        ...person, // ("spread" operator)
        id: uuid(),
        created: new Date(),
        superseded: new Date(Date.UTC(9999, 11, 1, 23, 59, 59)),
        start: new Date(),
        end: new Date(Date.UTC(9999, 11, 1, 23, 59, 59)),
        // We handle the "birthday" field like this to ensure it ends up properly in the datebase,
        // where it is stored as UTC time. Notice that the person from the form is given in local time
        birthday:
          person.birthday === null
            ? null
            : new Date(
                Date.UTC(
                  person.birthday.getFullYear(),
                  person.birthday.getMonth(),
                  person.birthday.getDate()
                )
              ),
        // We handle the "dead" field like this to ensure that values are set properly.
        // It is a bit of a hack, but I had to set the values of the deadOptions to strings rather than
        // booleans to get the combobox control to work, and then type of the "dead" property of the
        // PersonFormValues ends up as a string which will cause an error if we pass it to the
        // createPerson method of the agent
        dead:
          typeof person.dead === "object" || person.dead.toString() === ""
            ? null
            : person.dead.toString() === "true",
        surname: person.surname === "" ? null : person.surname,
        nickname: person.nickname === "" ? null : person.nickname,
        address: person.address === "" ? null : person.address,
        zipCode: person.zipCode === "" ? null : person.zipCode,
        city: person.city === "" ? null : person.city,
        category: person.category === "" ? null : person.category,
        description: person.description === "" ? null : person.description,
      };

      console.log(newPerson);

      createPerson(newPerson).then(() =>
        history.push(`/people/${newPerson.id}`)
      );
    } else {
      console.log("updating person..");

      let updatedPerson = {
        ...person, // ("spread" operator)
        birthday:
          person.birthday === null
            ? null
            : new Date(
                Date.UTC(
                  person.birthday.getFullYear(),
                  person.birthday.getMonth(),
                  person.birthday.getDate()
                )
              ),
        dead:
          typeof person.dead === "object" || person.dead.toString() === ""
            ? null
            : person.dead.toString() === "true",
        surname: person.surname === "" ? null : person.surname,
        nickname: person.nickname === "" ? null : person.nickname,
        address: person.address === "" ? null : person.address,
        zipCode: person.zipCode === "" ? null : person.zipCode,
        city: person.city === "" ? null : person.city,
        category: person.category === "" ? null : person.category,
        description: person.description === "" ? null : person.description,
      };

      console.log(updatedPerson);

      updatePerson(updatedPerson).then(() =>
        history.push(`/people/${person.id}`)
      );
    }
  }

  if (loadingInitial) return <LoadingComponent content="Loading person..." />;

  return (
    <Segment clearing>
      <Header content="Person Details" sub color="teal" />
      <Formik
        validationSchema={validationSchema}
        enableReinitialize
        initialValues={person}
        onSubmit={(values) => handleFormSubmit(values)}
      >
        {({ handleSubmit, isValid, isSubmitting, dirty }) => (
          <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
            <MyTextInput name="firstName" placeholder="First Name" />
            <MyTextInput name="surname" placeholder="Surname" />
            <MyTextInput name="nickname" placeholder="Nickname" />
            <MyTextInput name="address" placeholder="Address" />
            <MyTextInput name="zipCode" placeholder="Zip Code" />
            <MyTextInput name="city" placeholder="City" />
            <MyDateInput
              placeholderText="Birthday"
              name="birthday"
              timeCaption="time"
              dateFormat="MMMM d, yyyy"
            />
            <MyTextInput name="category" placeholder="Category" />
            <MySelectInput
              options={deadOptions}
              placeholder="Dead"
              name="dead"
            />
            <MyTextArea rows={3} placeholder="Description" name="description" />
            <Button
              disabled={isSubmitting || !dirty || !isValid}
              loading={isSubmitting}
              floated="right"
              positive
              type="submit"
              content="Submit"
            />
            <Button
              as={Link}
              to="/people"
              floated="right"
              type="button"
              content="Cancel"
            />
          </Form>
        )}
      </Formik>
    </Segment>
  );
});
