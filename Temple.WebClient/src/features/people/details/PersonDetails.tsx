import { observer } from "mobx-react-lite";
import React, { useEffect } from "react";
import { useParams } from "react-router-dom";
import LoadingComponent from "../../../app/layout/LoadingComponents";
import { useStore } from "../../../app/stores/store";
import PersonDetailedHeader from "./PersonDetailedHeader";
import PersonDetailedInfo from "./PersonDetailedInfo";
import PersonRelations from "./PersonRelations";

export default observer(function PersonDetails() {
  const { personStore } = useStore();
  const {
    selectedPerson: person,
    loadPerson,
    loadingInitial,
    clearSelectedPerson,
  } = personStore;
  const { id } = useParams<{ id: string }>();

  useEffect(() => {
    if (id) loadPerson(id);
    return () => clearSelectedPerson();
  }, [id, loadPerson, clearSelectedPerson]);

  if (loadingInitial || !person) return <LoadingComponent />;

  return (
    <>
      <PersonDetailedHeader person={person} />
      <PersonDetailedInfo person={person} />
      <PersonRelations />
    </>
  );
});
