import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import IconButton from "@mui/material/IconButton";
import CloseIcon from "@mui/icons-material/Close";

const SnackbarNotification = ({ open, setOpen, message, severity = "info" }) => {
  const handleClose = (event, reason) => {
    if (reason === "clickaway") return;
    setOpen(false);
  };

  return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      onClose={handleClose}
      anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
    >
      <Alert
        severity={severity}
        onClose={handleClose}
        sx={{
          backgroundColor: "#333",
          color: "#fff",
          width: "100%",
          display: "flex",
          alignItems: "center",
        }}
        icon={false} // Убираем иконку типа ("!" или "i"), можно убрать если хочешь оставить
        action={
          <IconButton
            size="small"
            aria-label="close"
            color="inherit"
            onClick={handleClose}
          >
            <CloseIcon fontSize="small" />
          </IconButton>
        }
      >
        {message}
      </Alert>
    </Snackbar>
  );
};

export default SnackbarNotification;
