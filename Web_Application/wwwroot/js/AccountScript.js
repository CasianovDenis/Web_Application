function showprofile() {
    
    
     if (profile.style.display == "none") {

         email.style.display = "none";
         password.style.display = "none";
         tfa.style.display = "none";
         verification.style.display = "none";
         disable_tfa.style.display = "none";
         enable_tfa.style.display = "none";
         profile.style.display = "block";

            } else {
                profile.style.display = "none";
            }
}

function showemail() {

  

    if (email.style.display === "none") {

        profile.style.display = "none";
        password.style.display = "none";
        tfa.style.display = "none";
        verification.style.display = "none";
        disable_tfa.style.display = "none";
        enable_tfa.style.display = "none";
        email.style.display = "block";
    
    } else {
        email.style.display = "none";
    }
}

function showpassword() {
   
   
    if (password.style.display === "none") {
        profile.style.display = "none";
        tfa.style.display = "none";
        email.style.display = "none";
        verification.style.display = "none";
        disable_tfa.style.display = "none";
        enable_tfa.style.display = "none";
        password.style.display = "block";
    } else {
        password.style.display = "none";
    }
}

function show2fa() { 
    if (tfa.style.display === "none") {
        profile.style.display = "none";       
        email.style.display = "none";
        password.style.display = "none";
        verification.style.display = "none";
        disable_tfa.style.display = "none";
        enable_tfa.style.display = "none";
        tfa.style.display = "block";
    } else {
        tfa.style.display = "none";
    }
}

function enabletfa() {
    if (enable_tfa.style.display === "none") {
        profile.style.display = "none";
        email.style.display = "none";
        password.style.display = "none";
        verification.style.display = "none";
        tfa.style.display = "none";
        disable_tfa.style.display = "none";
        enable_tfa.style.display = "block";
    } else {
        enable_tfa.style.display = "none";
    }
}

function disabletfa() {
    if (disable_tfa.style.display === "none") {
        profile.style.display = "none";
        email.style.display = "none";
        password.style.display = "none";
        verification.style.display = "none";
        disable_tfa.style.display = "block";
        enable_tfa.style.display = "none";
        tfa.style.display = "none";
    } else {
        disable_tfa.style.display = "none";
    }
}


