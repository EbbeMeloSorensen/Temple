import { observer } from 'mobx-react-lite';
import React from 'react';
import { List } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import PersonListItem from './PersonListItem';

// Her skal vi somehow bruge en state variabel sat af brugeren til at afgøre, om der skal sorteres
// på navn eller på created. Det skal svare til, hvad der blev brugt i List.Handler.Handle-metoden,
// hvilket igen skal svare til samme brugervalg
//
// Det spiller stadig ikke. Du har fået ryddet lidt op i hvordan klienten sorterer, men
// der er bøvl med hvordan API'en henter ting op. Hvorfor kommer Anders Buskjær Nielsen
// før Anders (uden efternavn).. Det er vel fordi det det OrderBy Then By ikke virker efter hensigten..

export default observer(function PersonList() {
    const {personStore} = useStore();
    const {sortedPeople} = personStore;

    return (
        <List divided>
            {sortedPeople.map(person => (
                <PersonListItem key={person.id} person={person} />
            ))}
        </List>
    )
})