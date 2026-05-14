import { useSearchParams } from "react-router-dom";

function ActivateAccountPage(){

    const [searchParams] = useSearchParams();

    const token = searchParams.get("token");

    console.log(token);
    
    return(
     <div style={{
         display: 'flex',
         flexDirection: 'column',
         justifyContent: 'center',
         alignItems: 'center',
         height: '100vh',
         textAlign: 'center',
         gap: '20px'
     }}>
        <h1>Activate Account</h1>
        <p>#TODO.</p>
     </div>
    )
}


export default ActivateAccountPage;