export const environment = {
  production: true,
  serverRoot: "https://packt-app-service-test.azurewebsites.net/api",
  endpoints: {
    auth: {
      signUp: "/users/signup",
      signIn: "/users/signIn",  
    },
    todos: "/todos"
  }
};
