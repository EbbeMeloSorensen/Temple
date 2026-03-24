import React from 'react';
import { Placeholder } from 'semantic-ui-react';

export default function PersonListItemPlaceholder() {
    return (
        <Placeholder fluid style={{ height: 10 }}>
            <Placeholder.Paragraph />
        </Placeholder>
    );
};
