import {
  MantineProvider,
  ColorSchemeProvider,
  AppShell,
  Header,
} from "@mantine/core";
import { QueryClient } from "react-query";
import { QueryClientProvider } from "react-query";
import { MovieCardGrid } from "./app/movie-card-grid";
import { DoubleNavbar } from "./app/nav";

const queryClient = new QueryClient();

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <MantineProvider>
        <AppShell
          // padding="md"
          navbar={<DoubleNavbar />}
          styles={(theme) => ({
            main: {
              backgroundColor:
                theme.colorScheme === "dark"
                  ? theme.colors.dark[8]
                  : theme.colors.gray[0],
            },
          })}
        >
          <MovieCardGrid />
        </AppShell>
      </MantineProvider>
    </QueryClientProvider>
  );
}
