import Welcome from "@/Welcome.tsx";
import Inbox from "@/Inbox.tsx";

function App() {
    const mailbox = window.location.pathname.split('/')[1];
    console.log(`Mailbox: ${mailbox}`);
    
    return (
        <>
        { !mailbox ? <Welcome /> : <Inbox /> }
        </>
    );
}

export default App
