import {makeAutoObservable} from "mobx";
import {PublicationsService as publicationsService} from "../services/publicationsService.js";

class PublicationsStore {
    publications = [];

    constructor() {
        makeAutoObservable(this);
    }

    async fetchMy() {
        try {
            const response = await publicationsService.fetchMy();
            this.publications = response.data;
        } catch (error) {
            console.log(error);
            return;
        }
    }
}

export const publicationsStore = new PublicationsStore();