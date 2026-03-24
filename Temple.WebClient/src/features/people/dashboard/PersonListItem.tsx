import React, { SyntheticEvent, useState } from 'react';
import { Link } from "react-router-dom";
import { Button, Item, List, Segment } from "semantic-ui-react";
import { Person } from "../../../app/models/person";
import { useStore } from '../../../app/stores/store';

interface Props {
    person: Person
}

export default function PersonListItem({person}: Props) {
    const {personStore} = useStore();
    const {deletePerson, loading} = personStore;
    const [target, setTarget] = useState('');

    function handlePersonDelete(e: SyntheticEvent<HTMLButtonElement>, id: string) {
        setTarget(e.currentTarget.name);
        deletePerson(id);
    }

    return (
        <List.Item>
            <List.Content>
                <List.Header as={Link} to={`/people/${person.id}`}>
                    {person.firstName} {person.surname}
                </List.Header>
            </List.Content>
        </List.Item>
    )
}