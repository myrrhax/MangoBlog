import {useState} from "react";

const useSecretText = (timeout) => {
    const [open, setOpen] = useState(false);

    const show = () => {
        if (open) return;
        setOpen(true);
        setTimeout(() => setOpen(false), timeout);
    }

    return [open, show];
}

export default useSecretText;