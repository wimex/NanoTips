import {useCreateMailboxMutation} from "@/redux/api.ts";

export default function Welcome() {
  const [createMailbox, {isLoading}] = useCreateMailboxMutation();

  function onLogin(event: React.MouseEvent<HTMLButtonElement>) {
    event.preventDefault();

    const guid = (document.getElementById('guid') as HTMLInputElement).value;
    if(!guid)
      return alert('Please enter a GUID');
    
    window.location.href = `/${guid}`;
  }
  
    async function onRegister(event: React.FormEvent<HTMLFormElement>) {
      event.preventDefault();

      const formData = new FormData(event.currentTarget);
      const openAiApiKey = formData.get('openai') as string;
      const postmarkApiKey = formData.get('postmark') as string;
      if (!openAiApiKey || !postmarkApiKey)
        return alert('Please enter both OpenAI and Postmark API keys');
      if (openAiApiKey.length < 160 || !openAiApiKey.startsWith('sk-'))
        return alert('This doesn\'t look like a valid OpenAI API key');
      if (postmarkApiKey.length < 32)
        return alert('This doesn\'t look like a valid Postmark API key');

      try {
        const result = await createMailbox({openAiApiKey, postmarkApiKey}).unwrap();
        console.log('Registration successful:', result);
        window.location.href = `/${result.mailboxId}`;
      } catch (error) {
        console.error('Registration failed:', error);
      }
    }
  
  return (
    <div className="flex flex-col items-center justify-center h-full">
      <h1 className="text-4xl font-bold mb-4">Welcome to NanoTips!</h1>
    <div className="flex space-x-8">
      <div className="w-1/2">
        <h2 className="text-2xl mb-2">Login</h2>
        <form className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="guid">GUID</label>
            <input className="w-full p-2 border rounded" type="text" id="guid" name="guid" required />
          </div>
          <button className="w-full bg-blue-500 text-white p-2 rounded hover:bg-blue-600" type="submit" onClick={onLogin}>Login</button>
        </form>
      </div>
      <div className="w-1/2">
        <h2 className="text-2xl mb-2">Register</h2>
        <form className="space-y-4" onSubmit={onRegister}>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="openai">OpenAI API key</label>
            <input className="w-full p-2 border rounded" type="text" id="openai" name="openai" required />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="postmark">Postmark API key</label>
            <input className="w-full p-2 border rounded" type="text" id="postmark" name="postmark" required />
          </div>
          <button className="w-full bg-green-500 text-white p-2 rounded hover:bg-green-600" type="submit" disabled={isLoading}>Register</button>
            <p className="text-sm text-gray-500 mt-2">
              After registering, you'll be redirected to your unique URL with a GUID. Bookmark that page for future access.
            </p>
        </form>
      </div>
      </div>
    </div>
  );
}