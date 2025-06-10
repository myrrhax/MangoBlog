import {useFormik} from "formik";
import {authStore} from "../stores/authStore.js";
import {useNavigate} from "react-router-dom";
import {mediaService} from "../services/mediaService.js";

const useRegistrationForm = (minDate, maxDate) => {
    const navigate = useNavigate();

    const loadAvatar = async (avatar) => {
        if (avatar) {
            try {
                const response = await mediaService.loadMedia(avatar, true);
                return response.data.id;
            } catch {
                authStore.setError('Avatar upload failed');
                return null;
            }
        }
    }

    const formik = useFormik({
        initialValues: {
            login: '',
            password: '',
            confirmPassword: '',
            avatar: null,
            firstName: '',
            lastName: '',
            birthDate: '',
            email: '',
        },
        validate: (values) => {
            const errors = {};
            if (!values.login) {
                errors.login = 'Required';
            }
            if (!values.password) {
                errors.password = 'Required';
            } else if (values.password.length < 6) {
                errors.password = 'Password must be at least 6 characters';
            }
            if (!values.confirmPassword) {
                errors.confirmPassword = 'Required';
            } else if (values.password !== values.confirmPassword) {
                errors.confirmPassword = 'Passwords do not match';
            }
            if (!values.firstName) {
                errors.firstName = 'Required';
            }
            if (!values.lastName) {
                errors.lastName = 'Required';
            }
            if (!values.birthDate) {
                errors.birthDate = 'Required';
            } else {
                const selectedDate = new Date(values.birthDate);
                if (selectedDate < minDate || selectedDate > maxDate) {
                    errors.birthDate = 'Please select a valid date between 13 and 100 years ago';
                }
            }
            return errors;
        },

        onSubmit: async (values) => {
            let avatarId = null;
            if (values.avatar) {
                avatarId = await loadAvatar(values.avatar);
                if (avatarId === null) { // Не удалось загрузить
                    return;
                }
            }
            const success = await authStore.register(
                values.login,
                values.password,
                values.email,
                values.firstName,
                values.lastName,
                values.birthDate,
                avatarId
            );
            if (success) {
                navigate('/');
            }
        },
    });

    const onPasswordChange = (value) => {
        formik.handleChange(value);
        authStore.validationErrors.password = null;
    }

    return [formik, onPasswordChange];
}

export default useRegistrationForm;