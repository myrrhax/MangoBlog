import { useState} from 'react';

const useNotificationSnackbar = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [message, setMessage] = useState('');

    return [isOpen, setIsOpen, message, setMessage];
}

export default useNotificationSnackbar;