import { observer } from "mobx-react-lite";
import React, { useState } from "react";
import { Button, Checkbox, Form, Header, Label, Radio } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";

export default observer(function PeopleFilters() {
    const {personStore: {setPredicate, sorting, setSorting}} = useStore();

    // Nogle states, vi gerne vil sende til personStore, når man klikker på Search-knappen
    const [nameFilter, setNameFilter] = useState('');
    const [categoryFilter, setCategoryFilter] = useState('');
    const [dead, setDead] = useState(false);
    const [notDead, setNotDead] = useState(false);
    const [deadUnspecified, setDeadUnspecified] = useState(false);
    const [sortingLocal, setSortingLocal] = useState(sorting);

    function handleClick() {
        setSorting(sortingLocal);
        setPredicate(nameFilter, categoryFilter, dead, notDead, deadUnspecified);
    }

    return (
        <>
            <Header icon='filter' attached color='teal' content='Filters' />

            <Header>Name</Header>
            <Label>Name contains</Label>
                <input value={nameFilter} onChange={e => setNameFilter(e.target.value)}
            />

            <Header>Dead</Header>
            <Form>
                <Form.Field>
                    <Checkbox
                        label='Yes'
                        defaultChecked={dead}
                        onChange={() => setDead(!dead)}
                    />
                </Form.Field>
                <Form.Field>
                    <Checkbox
                        label='No'
                        defaultChecked={notDead}
                        onChange={() => setNotDead(!notDead)}
                    />
                </Form.Field>
                <Form.Field>
                    <Checkbox
                        label='Unspecified'
                        defaultChecked={deadUnspecified}
                        onChange={() => setDeadUnspecified(!deadUnspecified)}
                    />
                </Form.Field>
            </Form>

            <Header>Sorting</Header>
            <Form>
                <Form.Field>
                    <Radio
                        label='Name'
                        name='radioGroup'
                        value='this'
                        checked={sortingLocal === 'name'}
                        onChange={() => setSortingLocal('name')}
                    />
                </Form.Field>
                <Form.Field>
                    <Radio
                        label='Created'
                        name='radioGroup'
                        value='that'
                        checked={sortingLocal === 'created'}
                        onChange={() => setSortingLocal('created')}
                    />
                </Form.Field>
            </Form>

            <Button
                floated="right"
                content='Search'
                onClick={() => handleClick()}
            />
        </>
    )
})