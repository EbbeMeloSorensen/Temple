import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import InfiniteScroll from "react-infinite-scroller";
import { Grid, Loader } from "semantic-ui-react";
import { PagingParams } from "../../../app/models/pagination";
import { useStore } from "../../../app/stores/store";
import PersonFilters from "./PersonFilters";
import PersonList from "./PersonList";
import PersonListItemPlaceholder from "./PersonListItemPlaceholder";

export default observer(function PersonDashboard() {
    const {personStore} = useStore();
    const {loadPeople, personRegistry, setPagingParams, pagination} = personStore;
    const [loadingNext, setLoadingNext] = useState(false);
  
    function handleGetNext() {
        setLoadingNext(true);
        setPagingParams(new PagingParams(pagination!.currentPage + 1))
        loadPeople().then(() => setLoadingNext(false));
    }

    useEffect(() => {
      if (personRegistry.size <= 1) loadPeople();
    }, [personRegistry.size, loadPeople])

    return (
        <Grid>
            <Grid.Column width='10'>
                {personStore.loadingInitial && !loadingNext ? (
                    <>
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                        <PersonListItemPlaceholder />
                    </>
                ) : (
                        <InfiniteScroll
                            pageStart={0}
                            loadMore={handleGetNext}
                            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
                            initialLoad={false}
                        >
                            <PersonList />
                        </InfiniteScroll>
                    )}
            </Grid.Column>
            <Grid.Column width='6'>
                <PersonFilters />
            </Grid.Column>
            <Grid.Column width={10}>
               <Loader active={loadingNext} />
            </Grid.Column>
        </Grid>
    )
})