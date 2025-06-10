import {TextField} from "@mui/material";
import React from "react";

const FormInputField = ({id, label, name, value, onChange, error, helperText,
                            type='',
                            isRequired = true}) => {
    return (
        <TextField
            margin="normal"
            fullWidth
            required={isRequired}
            id={id}
            label={label}
            name={name}
            type={type}
            value={value}
            onChange={onChange}
            error={error}
            helperText={helperText}
        />
    );
}

export default FormInputField;